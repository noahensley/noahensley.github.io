#!/usr/bin/env python


#set this equal to the path to your compiler executable
#or else use the -c command line option
COMPILER="./bin/Debug/net10.0/compiler-sharp.exe"

#can test only files in a specific dir
targetDir=""

#to run noninteractively, set this to False or use the -n command line option
interactive=False

import sys, os.path, getopt, subprocess, json


def error(msg):
    print(msg)
    if interactive:
        input("Press 'enter' to quit. ")
    sys.exit(0)

def done():
    if interactive:
        input("Press 'enter' to quit. ")
    sys.exit(0)

def run(inp: str):
    procargs = ["--run", ""]
    testdir = inp.split("\\")[-2]
    match testdir:
        case "tokjson":
            procargs[1] = "-tok-json"
        case "treejson":
            procargs[1] = "-tree-json"
        case "treebox":
            procargs[1] = "-tree-box"
        case "dotfile":
            procargs[1] = "-dotfile"
        case "typecheck":
            procargs[1] = "-type-check"
    P = subprocess.Popen([COMPILER,procargs[0],procargs[1],inp],stdout=subprocess.PIPE,encoding='utf-8',errors="ignore")
    o,e = P.communicate()
    if P.returncode != 0:
        return "null"
    else:
        return o

def compareJSON(actual,expected):
    if expected == None and actual == None:
        return True
        
    if expected == None and actual != None:
        return False
        
    if expected != None and actual == None:
        return False

    if type(actual) != list :
        print("Expected a list; got",type(actual))
        return False
        
    if len(actual) != len(expected):
        print("Expected list to have",len(expected),"tokens, but got",len(actual),"tokens")
        return False
    for i in range(len(expected)):
        atoken = actual[i]
        etoken = expected[i]
        if atoken["sym"] != etoken["sym"]:
            print("Token",i,": Expected sym to be",etoken["sym"],"but got",atoken["sym"])
            return False
        if atoken["line"] != etoken["line"]:
            print("Token",i,": Expected line to be",etoken["line"],"but got",atoken["line"])
            return False
        if atoken["lexeme"] != etoken["lexeme"]:
            print("Token",i,": Expected lexeme to be",etoken["lexeme"],"but got",atoken["lexeme"])
            return False
        if atoken.get("column") != etoken["column"]:
            return False
    return True

def compareOut(actual,expected):
    return actual == expected

opts,args = getopt.getopt(sys.argv[1:], "c:n:d:" )
for o,a in opts:
    if o == "-c":
        COMPILER=a
    elif o == "-n":
        interactive=False
    elif o == "-d":
        targetDir=str(a)
    else:
        assert False

inputdir=os.path.join(os.path.dirname(__file__),"tests","unit","inputs")
outputdir=os.path.join(os.path.dirname(__file__),"tests","unit","outputs")

if not os.path.exists(inputdir):
    error("Cannot find tests folder; it should be side-by-side with this harness.")

inputs={}
for dirname,dirs,files in os.walk(inputdir):
    for d in dirs:
        for subdirname,subdirs,files in os.walk(os.path.join(inputdir,d)):
            for f in files:
                if f.endswith(".txt"):
                    if d not in inputs.keys():
                        inputs[d] = [f]
                    else:
                        inputs[d] += [f]

total = 0
for dirname in inputs.keys():
    filelist = inputs[dirname]
    for i in range(len(filelist)):
        if targetDir and dirname != targetDir:
            break
        total += 1
        fname = filelist[i]
        print("Test",total,"(",dirname,"/",fname,")...")
        requiredOK = False      
        inputfile = os.path.join(inputdir,dirname,fname)
        output = run(inputfile)
        expectedfile = os.path.join(outputdir,dirname,fname)
        if "treejson" in dirname or "typecheck" in dirname: # these dirs have .json output files
            expectedfile = os.path.join("\\".join(expectedfile.split(".")[:-1]) + ".json")
        with open(expectedfile, encoding="utf-8-sig") as fp: # utf-8-sig strips BOM if present
            data = fp.read()
            if "tokjson" in dirname: # this dir has .json output inside a list;
                output = json.loads(output)
                expected = json.loads(data)
                requiredOK = compareJSON(output,expected)
            else:
                expected = data
                requiredOK = compareOut(output,expected)

        if not requiredOK:
            error(f"Mismatch: Expected: {expected}\tGot: {output}")


print("All required functionality OK")
done()
