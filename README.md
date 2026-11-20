# GoLight interpreter — ANTLR4 + C# + Next.js

An interpreter for **GoLight**, a subset of Go, with a web editor. You write code in the browser, and a C#/.NET API parses it with ANTLR4, runs it with a tree-walking interpreter, and returns the console output, the symbol table and the errors.

Course project for *Organización de Lenguajes y Compiladores 2* (Universidad de San Carlos de Guatemala, 1st semester 2025). Individual work. The second project compiles the same language to ARM64 assembly: **[go2arm64-compiler](https://github.com/AbdielOtzoy/go2arm64-compiler)**.

## What it supports

- Variables (`var`, `:=`) with types `int`, `float64`, `string`, `bool` and `rune`, plus type checking
- `if`, `switch`, `for` (classic, condition-only and `range`), `break`, `continue` and `return`
- Functions and methods on structs
- Slices and matrices (`[]T`, `[][]T`) and structs
- Built-ins: `append`, `len`, `strconv.Atoi`, `strconv.ParseFloat`, `strings.Join`, `slices.Index` and `reflect.TypeOf`
- Reports: symbol table, lexical, syntax and semantic errors with line and column, and the parse tree as SVG

## How it works

```text
Next.js editor (Monaco) ──POST /compile──► ASP.NET Core API
                                              ├─ ANTLR4 lexer + parser (backend/grammars/Language.g4)
                                              └─ CompilerVisitor: environments, symbols, structs, slices, calls
```

The main pieces are in [`backend/compiler`](backend/compiler): `Enviroment.cs` handles scopes, `Invocable.cs` and `Foreign.cs` handle user functions and built-ins, and `Struct.cs`/`Instance.cs` handle structs. `break`, `continue` and `return` are exceptions defined in `TransferValues.cs`.

## Run it

```bash
cd backend && dotnet watch run                      # .NET 9 SDK
cd frontend && pnpm install && pnpm dev              # Node.js + pnpm
```

## Docs (Spanish)

[Technical manual](docs/ManualTecnicoGoLigth.pdf) · [User manual](docs/ManualUsuarioGoLigth.pdf) · [Grammar](docs/gramaticaANTLR4.pdf)
