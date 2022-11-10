# Backend
Repositório do backend desenvolvido em .net core.

# Banco de Dados
O banco de dados é bem simples, basicamente duas tabelas que devem rodar no SQLServer. No diretório DATA coloquei dois arquivos, um BAK que pode ser utilizado para restaurar o banco completo com dados. A outra opção é rodar os scripts do arquivo .SQL para criar as tabelas onde quiser.

Após restaurar/rodar os scripts, haverá duas tabelas:

## Tabela Company
Armazena as informações básicas da empresa
- id
- symbol que o código do ativo na api Yahoo Finance
- friendlyName o código do ativo tratado sem número ou sufixo .SA
- currency que é a moeda do ativo
- companyLogo link para a logo do ativo (estou usando as imagens dos ativos disponíveis na bastter.com)
- type indica se uma ação é ON, PN ou UNIT (tratamento a partir do symbol da Yahoo Finance)

## Tabela CompanyPriceHistory
Armazena o histório de cotações de cada ativo
- id
- companyId é chave estrangeira para a Company
- date é a data da cotação de abertura
- openPrice é a cotação de abertura na data especificada
- d1VariationPercentage armazena o % de variação para a cotação da data imediatamente anterior
- firstPriceVariationPercentage armazena o % da variação da openPrice atual para a primeira cotação registrada no histórico.

A lógica do backend desenvolvido é que sempre constará os últimos 30 dias de cotações.

Detalhes a seguir.

# Backend
O projeto foi desenvolvido tendo em mente a separação de responsabilidades. Temos a seguinte estrutura

## Domain
Domain contém as definições dos models e das inferfaces que no projeto atual se restringe a interface ICompanyRepository
já que decidi implementar o padrão repository.

A ideia do repositóry é a total separação da camada de negócio da camada de dados. Dessa forma pode-se trocar a implementação de acesso 
aos dados e até mesmo a própria base de dados sem qualquer interferência no restante da aplicação.

## Infra
Camada de acesso aos dados, onde nosso CompanyRepository trata de toda a lógica de acesso aos dados da aplicação. Trata-se poucos endpoints simples, porém esta arquitetura é escalável e de fácil manutenção e organização.

## Api
Esta é a camada de negócio da aplicação, onde estará o core do negócio. As regras que calculam a variação percentual do ativo estará aqui.
Por serem poucas as operações necessárias na API, optei por desenvolver um único controller.

Adicionei o Swegger e ao rodar o projeto já será possível visualizar os endpoints para teste.

# Finalmente, Rodando o Backend
Após preparar as duas tabelas na base de dados (as tabelas podem estar inclusive completamente 
vazias e serem alimentadas pelo endpoint/updateHistory), será preciso configurar a string de conexão.

- Abra o arquivo config.json localizado na raíz do diretório guide.Domain.Api
- Altere a conexão de acordo com o seu banco de dados. A conexão configurada no meu local utilizava a autenticação integrada do windows.

Caso deseje, pode alterar a porta onde será alocada a aplicação da API (está configurada para subir na 5000). Para isto:

- Altere as configurações do arquivo launchSettings.json no diretório guide.Domain.Api/Properties.

Basta executar os seguintes comandos para iniciar o backend:

- cd guide.Domain.Api
- dotnet build
- dotnet run (deve ser executado dentro do diretório guide.Domain.Api ou haverá um erro por não encontrar o projeto para startar.)

A API ficará disponível na porta 5000.

Abra o navegador e acesse o seguinte URL: https://localhost:5000/swagger/index.html

Serão exibidos os endpoints disponibilizados na API.

Caso tenha restaurado a base com dados, verá que ela já possui dados dos seguintes ativos 

WEGE3
MDIA3
RADL3
BBDC4
HYPE3
PETR4
VALE3
FLRY3
PETR3
SAPR11
ITUB4


# Endpoint [POST] /guide/company/{symbol}/updateHistory
Novos tickers e históricos são adicionados pelo endpoint /updateHistory. Pode começar a base do zero se desejar, basta
chamar este endpoint passando os tickers desejados. E depois utilizar os outros endpoints para consumir os dados.

Este endpoint consulta usando a biblioteca RestSharp. O parse do JSON é feito utilizando 
a tradicional Library NewtonsoftJson.

Os outros endpoints apenas expõem os dados buscados do Yahoo Finance já em formato JSON e de mais fácil leitura,
pois relaciono a data com a cotação de abertura e já calculo previamente a variação entre d-1 e da cotação atual com todo
o período anterior.

# Endpoint [GET] /guide/company/{symbol}/info
Consulta simples a informações de um ativo. 

```
{
  "id": 1,
  "symbol": "WEGE3.SA",
  "friendlyName": "WEGE",
  "type": "ON",
  "currency": "BRL",
  "companyLogo": "https://bastter-storage.b-cdn.net/acao/WEGE.gif"
}
```
Essas informaões serão usadas para fazer o front no Flutter. Tomei a liberdade de usar imagens da Bastter.com, trabalhei lá e sei que essas imagens estão públicas para uso não comercial.

# Endpoint [GET] /guide/company/{symbol}/priceHistory
Consulta a lista de cotações do ativo nos últimos 30 pregões.

Experimento buscar algum ativo que não está na base, e depois incluir ele com o /updateHistory e tente novamente, os resultados serão diferentes.
E podem ser usados para simular as diferentes situções na solução de frontend.

# Endpoint [GET] /guide/company/{symbol}/lastPrice
Consulta a última cotação de abertura do ativo. Já traz a variação d-1 e para a primeira cotação do range de 30 progões.




