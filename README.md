# Hubdiário - Aplicação Web
Hubdiário é uma aplicação web desenvolvida para organizar e gerir informações da vida quotidiana. A aplicação permite criar temas personalizados e armazenar informações de maneira segura e acessível. Além disso, oferece funcionalidades de alerta por e-mail para garantir que informações importantes não sejam esquecidas.

# Funcionalidades
  - Criação de Conta: Permite que visitantes criem uma conta de acesso.
  - Autenticação: Entrada e saída de utilizadores autenticados no sistema.
  - Gestão de Perfil: Criação, leitura, atualização e eliminação de informações do perfil.
  - Temas e Categorias: Criação, edição e visualização de temas e suas respectivas categorias.
  - Itens e Alertas: Criação e edição de itens dentro das categorias e configuração de alertas por e-mail.
  - Alertas Recorrentes: Configuração de alertas com repetição em intervalos específicos (hora, diário, semanal, mensal, anual).

# Tecnologias Utilizadas
  - Backend: ASP.NET (C#)
  - Frontend: HTML, CSS, Bootstrap
  - Base de Dados: Microsoft SQL Server
  - Email: SMTP (Gmail)
  - Ferramentas: Microsoft Visual Studio, IIS

# Requisitos
Ambiente de Desenvolvimento: Microsoft Visual Studio Community 2022
.NET Framework: 4.7.2
Base de Dados: Microsoft SQL Server 2022
Servidor Web: Internet Information Services (IIS)
Conta de Email: Gmail para envio de alertas

# Instalação
1. Clone o repositório:
  - git clone https://github.com/StorMyth/hubdiario.git

2. Configure a base de dados:
  - Crie a base de dados no Microsoft SQL Server utilizando o script fornecido em /Database/CriarTabelas.sql.
  - Configure os Stored Procedures em /Database/ALL_Procedures.sql. Na pasta /Database/Stored Procedures estão todos os comandos em separado.

3. Configure o servidor SMTP:
  - Configure uma conta Gmail para o envio de emails.
  - Atualize as configurações de SMTP no web.config ou app.config conforme necessário.

4. Compile e execute a aplicação:
  - Abra o projeto hubdiario.sln no Visual Studio.
  - Configure o IIS para hospedar a aplicação.
  - Compile e execute a aplicação, através do /hubdiario/Default.aspx

# Uso
1. Acesso:
  - Acesse a aplicação através do navegador.
  - Registe uma nova conta ou faça login com uma conta existente.

2. Gestão de Temas e Categorias:
  - Crie e edite temas e categorias conforme necessário.
  - Adicione itens e configure alertas dentro das categorias.

# Estrutura do Projeto
- Content/: Arquivos CSS e recursos estáticos.
- Pages/: Páginas ASP.NET para interações do utilizador.
- Scripts/: Scripts JavaScript.
- Database/: Scripts SQL para criação e gestão da base de dados.
- README.md: Documentação do projeto.
