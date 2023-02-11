@namespace BioTonFMS.Expressions
@classname ExpressionParser
@using BioTonFMS.Expressions.AST

expression <AstNode>
    = a:additive (unparsed / "") { a }
    
unparsed
    = . #error{ "Couldn't parse expression to the end!" } 

additive <AstNode> -memoize
    = left:additive _ "+" _ right:multiplicative { new BinaryOperation(left, right, BinaryOperationEnum.Addition) }
    / left:additive _ "-" _ right:multiplicative { new BinaryOperation(left, right, BinaryOperationEnum.Subtraction) }
    / multiplicative

multiplicative <AstNode> -memoize
    = left:multiplicative _ "*" _ right:primary { new BinaryOperation(left, right, BinaryOperationEnum.Multiplication) }
    / left:multiplicative _ "/" _ right:primary { new BinaryOperation(left, right, BinaryOperationEnum.Division) }
    / primary

primary <AstNode> -memoize
    = var
    / "-" _ primary:primary { new UnaryOperation(primary, UnaryOperationEnum.Negation) }
    / "(" _ additive:additive _ ")" { new UnaryOperation(additive, UnaryOperationEnum.Parentheses) }
    
var <AstNode>
    = !"const" v:([a-z_]i+ [a-z_0-9]i*) { new Variable(v) }
    / literal
    
literal <AstNode>
    = "const" d:decimal { d }

decimal <AstNode>
    = value:("-"? [0-9]+ ("." [0-9]+)? ([eE] "-"? [0-9]+)?) { new Literal(value, LiteralEnum.Decimal) }

_ <int>
    = [ \t]* { 0 } 
