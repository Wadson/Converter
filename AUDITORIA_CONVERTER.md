# Auditoria do ConverPro

Data: 11/09/2026. Projeto auditado: `ConverPro.csproj` (MAUI .NET 10 para
`net10.0-windows10.0.19041.0`, Windows não empacotado).

## Estrutura e arquitetura

Há um único projeto MAUI, organizado em `Pages`, `ViewModels`, `Services` e
`Models`. Os serviços e o `MainViewModel` são singletons registrados por DI.
Os arquivos físicos não possuem duplicação de classes; as repetições vistas no
documento `CodigoEstrutura.md` são apenas uma cópia consolidada. Os textos
físicos inspecionados estão em UTF-8 e com acentos corretos.

## Problemas confirmados e correções

| Problema | Causa e impacto | Correção aplicada | Evidência/teste |
| --- | --- | --- | --- |
| Playlist repetida por item | A fila expandia a playlist, mas passava URLs ainda associadas a ela ao yt-dlp sem `--no-playlist`. Cada item podia reexpandir a playlist. | URLs de fila são canônicas (`watch?v=id`) e cada chamada adiciona `--no-playlist`. | Revisão de `YoutubeDownloadService` e `MediaService`; a linha de argumentos final contém `--newline --no-playlist`. |
| Processo poderia retornar antes das leituras | `WaitForExitAsync` era a única espera; as últimas linhas de stdout/stderr e seus handlers não eram sincronizados. | `ProcessRunner` espera o fechamento dos dois streams, limita a espera após cancelamento e remove handlers. | Revisão estática; compilação bloqueada externamente (abaixo). |
| Processo sem atividade | Não havia watchdog. | Monitor de cinco minutos, reiniciado por saída; encerra a árvore de processos e retorna timeout explícito. | Revisão estática. |
| Progresso pouco específico | Regex genérica capturava qualquer percentual. | Parsing restrito a linhas `[download]` e etapas legíveis para extração/mesclagem. | Revisão estática. |
| Contador/progresso de fila | O status era genérico e o progresso podia recuar. | Status por etapa, contador `n/total`, texto `etapa n de total — título` e cálculo monotônico; falhas avançam a fila. | Revisão estática. |
| Ferramentas reavaliadas por item | A playlist chamava a detecção em cada download. | Cache protegido por semáforo durante a sessão. | Revisão estática. |
| Ícone MAUI inválido | O projeto referenciava `appiconfg.svg`, inexistente. | `MauiIcon` usa somente o SVG oficial; o PNG oficial foi copiado como `Resources/Images/appicon_menu.png` e aplicado apenas ao cabeçalho lateral. | Revisão do csproj/XAML. |

## Tratamento de saída e pausa

O download só é considerado sucesso quando existe novo arquivo não vazio e não
temporário na pasta de destino. Ao pausar, o token cancela o processo e o item
é marcado como `Pausado`; itens concluídos continuam fora da seleção de
retomada. Falhas de item são registradas e a fila avança para o próximo.

## Compilação e publicação

`dotnet --info` confirmou SDK 10.0.401 e runtime `win-x64`. `dotnet restore`
foi concluído com sucesso. As compilações Debug e Release não puderam concluir
porque processos externos `Microsoft.UI.Xaml.Markup.Compiler` deixaram
`obj/.../input.json` e `intermediatexaml/ConverPro.dll` mapeados/abertos. O
erro ocorreu antes da validação C# final e também afetou artefatos já presentes
no diretório de trabalho. Para não apagar artefatos modificados pelo usuário,
eles não foram limpos forçadamente.

Por isso, testes automatizados, teste real do YouTube, validação visual e a
publicação em `D:\Publicacao\Converter` permanecem pendentes. Depois de
fechar o processo/IDE que bloqueia `obj`, executar:

```powershell
dotnet restore
dotnet build -c Debug
dotnet test -c Debug
dotnet build -c Release
dotnet test -c Release
dotnet publish .\ConverPro.csproj -c Release -f net10.0-windows10.0.19041.0 -r win-x64 --self-contained true -p:WindowsPackageType=None -o D:\Publicacao\Converter
```

Não foi declarado sucesso de publicação nem de teste real devido a essa
limitação verificável do ambiente.

## Adendo de publicação

O bloqueio era específico ao diretório de trabalho em `D:\Projetos\Converter`.
Uma cópia temporária, sem `bin`, `obj` ou `.git`, foi compilada em
`C:\CodexBuild\ConverterPublishSource`, preservando o projeto original no
local solicitado. Resultados verificados:

* `dotnet build -c Debug --no-restore --disable-build-servers -m:1`: sucesso.
* `dotnet build -c Release --no-restore --disable-build-servers -m:1`: sucesso.
* `dotnet test -c Debug --no-build --disable-build-servers`: sucesso; a solução
  não possui projeto de testes descobrível.
* Publicação concluída com:

```powershell
dotnet publish ConverPro.csproj -c Release -f net10.0-windows10.0.19041.0 -r win-x64 --self-contained true -p:WindowsPackageType=None -p:PublishTrimmed=false --disable-build-servers -o D:\Publicacao\Converter
```

O executável publicado é `D:\Publicacao\Converter\ConverPro.exe`. Ele foi
iniciado diretamente e permaneceu ativo por oito segundos, validando a abertura
sem dependência do diretório de código-fonte. O processo de smoke test foi
encerrado após a verificação.
