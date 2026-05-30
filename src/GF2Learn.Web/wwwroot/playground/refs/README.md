# Playground assembly whitelist

Place pre-built `.dll` files here and register them in `playground.json`.

Example (Newtonsoft.Json):

```bash
dotnet new console -o /tmp/refpack -f net10.0
cd /tmp/refpack
dotnet add package Newtonsoft.Json
dotnet publish -c Release
cp bin/Release/net10.0/publish/Newtonsoft.Json.dll <repo>/src/GF2Learn.Web/wwwroot/playground/refs/
```

Reference in markdown:

```markdown
:::code-playground
refs: Newtonsoft.Json
```csharp
// your code
```
expected: ...
:::
```
