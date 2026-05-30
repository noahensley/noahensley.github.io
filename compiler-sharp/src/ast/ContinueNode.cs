using ASM;
public class ContinueNode : StmtNode
{
    Token token;
    private ContinueNode(Token token)
    {
        this.token = token;
    }

    public static bool canParse(Tokenizer T)
    {
        return T.peek() == TokenSymbols.CONTINUE;
    }

    public static new ContinueNode parse(Tokenizer T)
    {
        var token = T.expect(TokenSymbols.CONTINUE);
        return new ContinueNode(token);
    }

    public override void setType()
    {
        return; // no type
    }

    public override List<TreeNode> getChildren()
    {
        return new List<TreeNode>();
    }

    public override void typeCheck()
    {
        return; // no type
    }

    public override void genCode()
    {
        var p = this.parent;
        while (p != null)
        {
            LoopNode? L = p as LoopNode;
            if (L != null)
            {
                Asm.emit(
                    new Comment($"Continue at {this.token}"),
                    new OpJmp(L.test)
                );
                break;
            }                 
            else
            {
                p = p.parent;
            }
        }
        if (p == null)
            Utils.error(new InvalidExpression($"Got a breakpoint outside of a loop on line {this.token.line}" ));
    }

}