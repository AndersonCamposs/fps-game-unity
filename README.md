# Jogo FPS - Unity

Este é um projeto de jogo de tiro em primeira pessoa (FPS) desenvolvido na engine Unity como parte das atividades acadêmicas. O projeto foi estruturado utilizando boas práticas de controle de versão, garantindo um repositório limpo e otimizado.

## 🛠️ Tecnologias e Recursos Utilizados
* **Engine:** Unity 6 (Versão 6.4 / 6000.4.11f1).
* **Linguagem:** C# para a lógica dos scripts, inteligência artificial dos inimigos, gerenciamento de menus e controles do jogador.
* **IDE:** Visual Studio Code / Visual Studio.

---

## 📋 Requisitos Mínimos
Para garantir que o projeto abra sem erros de compilação ou perda de referências, certifique-se de cumprir os seguintes requisitos:
* **Versão da Engine:** Unity 6 (Especificamente a versão **6000.4.11f1** ou superior). 
* **Editor de Código:** Visual Studio ou VS Code configurado com suporte para scripts .NET/C#.
* **Espaço em Disco:** Aproximadamente 2GB a 4GB livres (a Unity recriará a pasta `Library` localmente ao abrir o projeto pela primeira vez).

---

## 🚀 Como Baixar e Rodar o Projeto

Para abrir e testar o projeto na sua máquina, siga os passos abaixo:

### 1. Clonar o Repositório
Abra o seu terminal (ou Git Bash) e clone o projeto usando o comando:

```bash
git clone https://github.com/AndersonCamposs/fps-game-unity.git
```
### 2. Abrir no Unity Hub
Como o repositório contém apenas os arquivos essenciais do projeto (como `Assets` e `ProjectSettings`), o próprio Unity se encarregará de gerar os arquivos temporários locais.

1. Abra o **Unity Hub**.
2. Clique no botão **Add** (ou *Add project from disk* no canto superior direito).
3. Navegue até a pasta onde você clonou/extraiu o projeto e selecione a pasta raiz (a pasta principal que contém o arquivo `.gitignore`).
4. Certifique-se de que selecionou a versão do editor **6000.4.11f1** no seletor de versões e clique no nome do projeto para abrir.
*Nota: A primeira inicialização pode demorar alguns minutos, pois a Unity estará reconstruindo os arquivos locais necessários.*

### 3. Como Localizar e Rodar a Cena Correta (GameScene)
Se o editor da Unity abrir com uma tela vazia ou uma cena padrão, siga os passos abaixo para carregar o jogo:

1. Na parte inferior da tela do editor, localize a aba **Project** (Geralmente fica ao lado da aba *Console*).
2. Dentro da aba Project, você verá a pasta principal chamada **Assets**. Clique nela.
3. Entre na pasta chamada **Scenes** dando um duplo clique.
4. Dentro dela, localize o arquivo da cena principal chamado **GameScene** (ele possui o ícone da logo da Unity).
5. Dê um **duplo clique em GameScene** para carregar o cenário, a interface e os elementos do jogo na sua tela.
6. Com a cena devidamente carregada, clique no botão **Play** (ícone de reprodução `▶️`) centralizado no topo do editor para testar e jogar!
