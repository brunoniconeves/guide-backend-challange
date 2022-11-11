# ATENÇÃO!
Como o teste finalizava as 16h eu interrompi o trabalho na branch principal para não afetar avaliação. Contúdo, tive MUITAS restrições de tempo nas 48h do exame e resolvi finalizar a minha ideia de desenvolvimento na branch V2Challange (tanto frontend quanto backend). Ainda que não levem em conta este desenvolvimento na segunda branch, está mais perto da minha visão final de app e implementa o gráfico em fl_chart e melhorias no fluxo geral de UI/UX. Caso tenham interesse em ver a tarefa finalizada, basta baixar o código da branch V2Challange usando. A única coisa que ficou faltando foi implementar código nativo, ai levaria ainda mais tempo do que algumas horas além do prazo.

- git switch V2Challange


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
Armazena o histório de cotações de cada ativoCancel changes
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
PETR4
PETR3
ITUB4
FLRY3
MDIA3


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
Consulta a lista de cotações do ativo nos últimos 30 pregões. Cada objeto representa um dia com a respectiva cotação e os dados de variação já calculados.

```
[
  {
    "id": 911,
    "companyId": 9,
    "date": "2022-11-09T00:00:00",
    "openPrice": 30.520000457763672,
    "d1VariationPercentage": 0.46,
    "firstPriceVariationPercentage": -6.95
  },
  {
    "id": 910,
    "companyId": 9,
    "date": "2022-11-08T00:00:00",
    "openPrice": 30.3799991607666,
    "d1VariationPercentage": -3.22,
    "firstPriceVariationPercentage": -7.38
  },
  ...
]
```

Experimento buscar algum ativo que não está na base, e depois incluir ele com o /updateHistory e tente novamente, os resultados serão diferentes.
E podem ser usados para simular as diferentes situções na solução de frontend.

# Endpoint [GET] /guide/company/{symbol}/lastPrice
Consulta a última cotação de abertura do ativo. Já traz a variação d-1 e para a primeira cotação do range de 30 progões. O objeto representa o dia da última cotação na base com a respectiva cotação e os dados de variação já calculados.

```
{
  "id": 911,
  "companyId": 9,
  "date": "2022-11-09T00:00:00",
  "openPrice": 30.520000457763672,
  "d1VariationPercentage": 0.46,
  "firstPriceVariationPercentage": -6.95
}
```




# Informações Adicionais

Não foi possível, por restrição de tempo, implementar a tela de código nativo. Optei por mostrar o melhor possível meus conhecimentos em Flutter, implementando, na melhor forma possível:

- uso de gestão de estados com GetX
- uso de rotas nomeadas, bindings, middlaware e passagem de parâmetros com GetX
- integração com a API por meio do DIO e injeção de dependências
- uso de animação para fazer uma interface amigável
- design fora da caixa utilizando curvas no header
- implementação do gráfico na biblioteca solicitada fl_chart

Infelizmente, como podem notar o tempo foi uma grande restrição, pois estou trabalhando normalmente e com muita demanda atualmente.(Tive uma entrega de um novo app mobile essa semana também).

Creio ter gastado cerca de 16 horas construindo o projeto do zero. 

Vejam que há um endpoint de obter informações da companhia e outro para obter somente o último preço.

A ideia era ter feito uma página mais elaborada para o Ativo, com informações que buscaria de outras fontes como Bastter.com ou de outros sites com dados abertos. Porém não houve tempo. 

# FEEDBACK do Challange
Gostei do desafio, implementei coisas rotineiras e coisas que não havia implementado ainda (nunca havia usado a fl_chart, por exemplo). Também acho que esse tipo de avaliação avalia o candidato de forma mais honesta que um code challange de 30 minutos com um problema totalmente fora do dia a dia do profissional. Este desafio foi um caso real com aplicação no mundo real.

Tenho uma sugestão quanto ao prazo. O prazo está ok se eu estivesse desempregado, porém para quem está empregado se torna um pouco cruel.

Uma terefa mais extensa como essa, acho que poderiam passar para fazer no final de semana (no mesmo prazo em horas). Seria mais ou menos o mesmo prazo, mas para o desenvolvedor que está empregado é mais factível realizar...e mais correto para concorrer com outros que podem ter mais tempo livre durante a semana.

Eu corri alguns riscos no meu trabalho para poder cumprir a missão
