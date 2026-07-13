# Keycloak — TripFlow

Este diretório contém a infraestrutura local do Keycloak usada pelo backend TripFlow.

## Tema de login customizado

O tema `tripflow` fica em:

```
infra/keycloak/themes/tripflow/
```

Ele é montado no container via `docker-compose.yml`:

```
./infra/keycloak/themes/tripflow:/opt/keycloak/themes/tripflow
```

## Subir o Keycloak

Na raiz do repositório backend:

```bash
docker compose up -d keycloak-db keycloak
```

Aguarde o Keycloak ficar disponível em `http://localhost:8080`.

## Ativar o tema no realm

1. Acesse o Admin Console: `http://localhost:8080`
2. Login admin:
   - usuário: `admin`
   - senha: `admin`
3. Selecione o realm `tripflow`
4. Vá em **Realm settings** → aba **Themes**
5. Em **Login theme**, selecione `tripflow`
6. Clique em **Save**

## Validar

Abra a tela de login do realm:

```
http://localhost:8080/realms/tripflow/protocol/openid-connect/auth?client_id=tripflow-admin-portal&redirect_uri=http://localhost:5173&response_type=code&scope=openid
```

Você deve ver:

- layout em duas colunas no desktop
- card branco centralizado à direita
- painel institucional navy/blue à esquerda
- botão azul `Sign In`
- mensagens de erro estilizadas

## Recarregar o tema após alterações

Se você editar arquivos do tema:

```bash
docker compose restart keycloak
```

Em `start-dev`, o Keycloak normalmente recarrega templates sem rebuild completo, mas reiniciar garante que CSS/FTL novos sejam aplicados.

## Estrutura do tema

```
tripflow/
  login/
    theme.properties
    template.ftl
    login.ftl
    resources/
      css/styles.css
      img/tripflow-icon.svg
```

## Observações

- O tema usa `parent=keycloak` e sobrescreve apenas o necessário para login.
- Fluxos como reset de senha continuam usando templates padrão do parent, salvo se forem customizados depois.
- Se existir export/import de realm no futuro, configure `"loginTheme": "tripflow"` no JSON do realm.
