# arriel-chat

Este projeto é constituído de dois projetos: arriel.chat.client e arriel.chat.server

O arriel.chat.server vai sempre escutar 127.0.0.1:2502 e vai ser responsável por escutar e tratar as solicitações feitas pelo arriel.chat.client. Ele foi preparado para aceitar alguns comandos passados com / antes, sendo necessário no chat client digitar /help após estar conectado para ter um breve walkthrough de como são os comandos.

O arriel.chat.client tem um campo para inserir o ip do servidor e um apelido para entrar no servidor. Ao registrar, o servidor mantém uma lista dos clientes que estão conectados e ele abre uma thread para cada um. A partir daí, é só digitar as mensagens e apertar ENTER ou clicar em ENVIAR para que a mensagem seja enviada ao server e tratada.

Nem toda mensagem é broadcasted para todos que estão na sala. Apenas as gerais. Isto vai ficar claro durante o uso.

Para testar os aplicativos, basta acessar nos links: https://drive.google.com/file/d/132zwDhJ2hs0IgbsDjfNDM2OABpOsok3Q/view?usp=sharing para o arriel.chat.client e https://drive.google.com/file/d/1nSWMS1lBmhhu9hAOBhAY5wnCJajFxTaP/view?usp=sharing para o arriel.chat.server. 

Os executáveis foram deployed embarcando DLL necessárias, o que ocasionou que ficassem grandes em tamanho, mas não necessariamente é a prática comum, fiz dessa forma para facilitar os testes.

Os códigos são simples e não exigiram a criação de centenas de pastas e classes e projetos para atingir o fim proposto no PDF. Na vida real, os códigos são bem mais modularizados do que isso.

O código pode ser visto aqui no git.

Yours,
Pablo de Oliveira