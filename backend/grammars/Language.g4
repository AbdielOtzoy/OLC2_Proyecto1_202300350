grammar Language;

program: (declaration NL?)*;

declaration: varDeclaration | statement;

varDeclaration: 'var' ID TYPE ('=' expr)?
    | ID ':=' expr;

statement: expr # ExprStmt
    | 'fmt.Println' '(' exprList ')' # PrintStmt
    | '{' declaration* '}' # BlockStmt
    | 'if' '(' expr ')' statement ('else' statement)? # IfStmt
    | 'switch' '(' expr ')' '{' caseClauseStmt* '}' # SwitchStmt;  

caseClauseStmt: 'case' expr ':' declaration*  # CaseClause
    | 'default' ':' declaration*          # DefaultClause;
exprList: expr (',' expr)*;

expr:
    '(' expr ')'                           # Parens
    | ID                                    # Identifier
    | BOOL                                  # Bool
    | FLOAT                                 # Float
    | STRING                                # String
    | INT                                   # Number
    | RUNE                                  # Rune
    | '!' expr                              # Not
    | '-' expr                              # Negate
    | expr '%' expr                         # Mod
    | expr op = ('*' | '/') expr      # MulDiv
    | expr op = ('+' | '-') expr            # AddSub
    | expr op = ('>' | '<' | '>=' | '<=') expr # Relational
    | expr op = ('==' | '!=') expr          # Equality
    | expr op = ('&&' | '||') expr          # Logical
    | expr '=' expr                           # Assign
    | ID '+=' expr                          # AddAssign
    | ID '-=' expr                          # SubAssign;

INT: [0-9]+;
BOOL: 'true' | 'false';
FLOAT: [0-9]+ '.' [0-9]+;
STRING: '"' ~[\r\n"]* '"';
RUNE: '\'' (ESCAPE | ~['\\]) '\'';

fragment ESCAPE: '\\' [nrt0'\\];

TYPE: 'int' | 'float64' | 'bool' | 'string' | 'rune';
ID: [a-zA-Z_][a-zA-Z_0-9]*;

WS: [ \t]+ -> skip;
NL: [\r\n]+ -> skip;

COMMENT: '//' ~[\r\n]* -> skip;
MULTILINE_COMMENT: '/*' .*? '*/' -> skip;
