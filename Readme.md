# Hangfire Server/Client

Esta poc mostra o funcionamento do hangfire com server e client separados entre outras features como:

  - Eventos de filters no processamento da mensagem
  - Dashboard com gestão de filas
  - Autenticação do dashboard

### Tecnologias

Foi usado as seguintes ferramentas:

* [NetCore 3.1]
* [Serilog] - Gestão de logs da aplicação
* [Swagger] - Interface de descrição de apis Restful.
* [Entity] - ORM de banco de dados
* [Hangfire] - Processa eventos em background
* [Mysql] - Banco de dados SQL

### Instalação

Existe 2 caminhos para executar a aplicação: 

#### Caminho 1 - Tendo um banco de dados Mysql

Para isto basta configurar a conectionstring do hangfire localizada em 
 POC.ServiceAPI/ConfigFiles/hangfire-config.json
 POC.ServiceWorker/ConfigFiles/hangfire-config.json


#### Caminho 2 - Executando via docker  

Com o docker instalado basta entrar na pasta do projeto via cmd e executar o comando

```sh
$ docker-compose -f docker-compose.yml -f mysql.yml -f configuration.yml up --build
```

