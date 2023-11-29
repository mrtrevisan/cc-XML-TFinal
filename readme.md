# Trabalho final - XML
### Professor Giovani Librelotto 
  
## Instruções de instalação:
### Instale o dotnet-sdk versão 8:

* No macOS: 
```
brew install --cask dotnet-sdk
```
* No linux:
<br>Siga as instruções no site oficial do .NET de acordo à sua distribuição

* No windows:
<br>Instale com instalador acessível via site oficial do .NET. No momento da escrita deste, a versão 8.0 estável não está disponível via Winget

### Verifique se a versão instalada é a 8.0
```
dotnet --version
```

### Executando com outra versão do .NET:

* Pode-se executar o programa usando outra versão do dotnet, mas isso pode gerar resultados indesejados!

* Edite as linhas `<TargetFramework>` e `<LangVersion>` no arquivo .csproj na pasta /App:

### Executando o programa:

* Clone este repositório usando https ou ssh (recomendado).

* Navegue até a pasta raiz do repositório (via shell). Em seguida rode o comando:
```
cd App
dotnet run
```

* Se tudo correr bem, você deverá ver a saída do programa no terminal, demonstrando o passo a passo da execução do programa e os resultados das queries.

### Estrutura do projeto:

* O código fonte está no arquivo /App/Program.cs e na pasta /App/class.

* Os arquivos xml de entrada estão na pasta /App/xml

* Os arquivos Json e o Json Schema gerados estarão na pasta /App/json após a execução do programa.

* Os arquivos html estarão na pasta /App/html e o index.html na pasta /App.
