<#import "template.ftl" as layout>
<@layout.registrationLayout displayMessage=false displayInfo=false; section>
    <#if section = "header">
        <#-- styles loaded via theme.properties -->
    <#elseif section = "form">
        <div class="tripflow-page">
            <aside class="tripflow-brand" aria-hidden="false">
                <div class="tripflow-brand__content">
                    <div class="tripflow-logo tripflow-logo--dark tripflow-logo--xl">
                        <img
                            class="tripflow-logo__icon"
                            src="${url.resourcesPath}/img/tripflow-icon.svg"
                            alt=""
                        />
                        <span class="tripflow-logo__text">
                            <span class="tripflow-logo__trip">TRIP</span><span class="tripflow-logo__flow">FLOW</span>
                        </span>
                    </div>
                    <h1 class="tripflow-brand__headline">Sua agência de viagens, conectada e inteligente.</h1>
                    <p class="tripflow-brand__text">
                        Centralize clientes, cotações, propostas, pagamentos e milhas em uma plataforma criada para simplificar sua operação e vender melhor.
                    </p>
                    <ul class="tripflow-brand__list">
                        <li>Atendimento e vendas em um só lugar</li>
                        <li>Propostas profissionais em poucos minutos</li>
                        <li>Gestão de pagamentos, milhas e clientes</li>
                    </ul>
                </div>
            </aside>

            <main class="tripflow-login">
                <div class="tripflow-card">
                    <div class="tripflow-card__header">
                        <div class="tripflow-logo tripflow-logo--light tripflow-logo--md">
                            <img
                                class="tripflow-logo__icon"
                                src="${url.resourcesPath}/img/tripflow-icon.svg"
                                alt=""
                            />
                            <span class="tripflow-logo__text">
                                <span class="tripflow-logo__trip">TRIP</span><span class="tripflow-logo__flow">FLOW</span>
                            </span>
                        </div>

                        <h2 class="tripflow-card__title">Bem-vindo à TripFlow</h2>
                        <p class="tripflow-card__subtitle">Entre para continuar sua operação</p>
                    </div>

                    <#if message?has_content && (message.type != 'warning' || !isAppInitiatedAction??)>
                        <div class="tripflow-alert" role="alert">
                            ${kcSanitize(message.summary)?no_esc}
                        </div>
                    </#if>

                    <#if realm.password>
                        <form id="kc-form-login" action="${url.loginAction}" method="post">
                            <#if !usernameHidden??>
                                <div class="tripflow-field">
                                    <label for="username" class="tripflow-label">
                                        <#if !realm.loginWithEmailAllowed>
                                            Usuário
                                        <#elseif !realm.registrationEmailAsUsername>
                                            Usuário ou e-mail
                                        <#else>
                                            E-mail
                                        </#if>
                                    </label>
                                    <input
                                        tabindex="1"
                                        id="username"
                                        class="tripflow-input"
                                        name="username"
                                        value="${(login.username!'')}"
                                        type="text"
                                        autofocus
                                        autocomplete="username"
                                        aria-invalid="<#if messagesPerField.existsError('username','password')>true</#if>"
                                    />
                                    <#if messagesPerField.existsError('username','password')>
                                        <span class="tripflow-field-error" id="input-error">
                                            ${kcSanitize(messagesPerField.getFirstError('username','password'))?no_esc}
                                        </span>
                                    </#if>
                                </div>
                            </#if>

                            <div class="tripflow-field">
                                <label for="password" class="tripflow-label">Senha</label>
                                <input
                                    tabindex="2"
                                    id="password"
                                    class="tripflow-input"
                                    name="password"
                                    type="password"
                                    autocomplete="current-password"
                                    aria-invalid="<#if messagesPerField.existsError('username','password')>true</#if>"
                                />
                            </div>

                            <div class="tripflow-row">
                                <#if realm.rememberMe && !usernameHidden??>
                                    <label class="tripflow-checkbox" for="rememberMe">
                                        <input
                                            tabindex="3"
                                            id="rememberMe"
                                            name="rememberMe"
                                            type="checkbox"
                                            <#if login.rememberMe??>checked</#if>
                                        />
                                        Lembrar-me
                                    </label>
                                <#else>
                                    <span></span>
                                </#if>

                                <#if realm.resetPasswordAllowed>
                                    <a class="tripflow-link" tabindex="5" href="${url.loginResetCredentialsUrl}">
                                        Esqueceu a senha?
                                    </a>
                                </#if>
                            </div>

                            <input
                                type="hidden"
                                id="id-hidden-input"
                                name="credentialId"
                                <#if auth.selectedCredential?has_content>value="${auth.selectedCredential}"</#if>
                            />

                            <div id="kc-form-buttons">
                                <button
                                    tabindex="4"
                                    class="tripflow-button"
                                    name="login"
                                    id="kc-login"
                                    type="submit"
                                >
                                    Entrar
                                </button>
                            </div>
                        </form>
                    </#if>

                    <#if realm.password && realm.registrationAllowed && !registrationDisabled??>
                        <div class="tripflow-register">
                            Não tem conta?
                            <a class="tripflow-link" href="${url.registrationUrl}">Cadastre-se</a>
                        </div>
                    </#if>

                    <p class="tripflow-footer">
                        <svg class="tripflow-footer__icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
                            <path d="M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10z"/>
                        </svg>
                        Protegido pela identidade TripFlow
                    </p>
                </div>
            </main>
        </div>
    </#if>
</@layout.registrationLayout>
