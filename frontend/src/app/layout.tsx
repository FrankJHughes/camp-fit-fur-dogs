'use client';

import { useState, useEffect, useRef } from 'react';
import { ActionsCard } from '@/lib/components/ActionsCard';
import { identify } from '@/api/identity/identify';
import { login } from '@/api/identity/login';
import { logout } from '@/api/identity/logout';
import { IdentityContext } from '@/lib/identity/identityContext';

import './globals.css';

export default function RootLayout({ children }: { children: React.ReactNode }) {
  // const { isAuthenticated, isUnavailable, user } = useIdentity();

  const [identityHeaderStable, setIdentityHeaderStable] = useState('Anonymous');
  const isAuthenticated = identityHeaderStable !== "Anonymous";

  // Layer refs
  const resultLayerRef = useRef<HTMLSpanElement>(null);
  const loadingLayerRef = useRef<HTMLSpanElement>(null);
  const statusLayerRef = useRef<HTMLSpanElement>(null);

  // Track which layer is currently opaque
  const previousLayerRef = useRef<HTMLSpanElement | null>(null);

  useEffect(() => {
    previousLayerRef.current = resultLayerRef.current;
    handleIdentify();
  }, []);

  //
  // Transition helper
  //
  function transitionTo(layer: HTMLSpanElement, text: string) {
    const previous = previousLayerRef.current!;
    layer.textContent = text;

    previous.classList.remove("opaque");
    previous.classList.add("transparent");

    layer.classList.remove("transparent");
    layer.classList.add("opaque");

    previousLayerRef.current = layer;
  }

  //
  // Action handler
  //

  async function handleIdentityAction(
    load: () => Promise<{ success: boolean, data: string }>,
    loadingMessage: string,
    getResultStatusMessage: (result: { success: boolean, data: string }) => string,
    interceptResult: (result: { success: boolean, data: string }) => boolean
  ) {
    const activeElement = (document.activeElement as HTMLElement | null);
    activeElement?.blur();

    const loadingLayer = loadingLayerRef.current!;
    const statusLayer = statusLayerRef.current!;
    const resultLayer = resultLayerRef.current!;

    // Result → Loading
    transitionTo(loadingLayer, loadingMessage ?? "Loading…");

    const result = await load();

    // Loading → Status
    transitionTo(statusLayer, getResultStatusMessage(result));
    statusLayer.classList.remove(result.success ? "failed" : "succeeded");
    statusLayer.classList.add(result.success ? "succeeded" : "failed");

    // Status → Result
    setTimeout(() => {
      const keepGoing = interceptResult(result);
      if (!keepGoing) {
        return;
      }
      transitionTo(resultLayer, result.data ?? "Anonymous");
    }, 2000);
  }

  //
  // Login handler
  //
  async function handleLogin() {
    handleIdentityAction(
      async () => {
        const returnUrl = window.location.href;
        const loginResult = await login(returnUrl);
        return { success: loginResult.success, data: loginResult.success ? loginResult.data.nextUrl : "" };
      },
      'Logging In...',
      (result: { success: boolean, data: string }) => result.success ? "Redirecting..." : "Login Failed",
      (result: { success: boolean, data: string }) => {
        if (result.success) {
          window.location.href = result.data;
        }
        return !result.success;
      }
    );
  }

  //
  // Logout handler
  //
  async function handleLogout() {
    handleIdentityAction(
      async () => {
        const returnUrl = window.location.href;
        const logoutResult = await logout(returnUrl);
        return { success: logoutResult.success, data: logoutResult.success ? logoutResult.data.nextUrl : "" };
      },
      'Logging Out...',
      (result: { success: boolean, data: string }) => result.success ? "Redirecting..." : "Logout Failed",
      (result: { success: boolean, data: string }) => {
        if (result.success) {
          window.location.href = result.data;
        }
        return !result.success;
      }
    );
  }

  //
  // get identity handler
  //
  async function handleIdentify() {
    return handleIdentityAction(
      async () => {
        const result = await identify();
        return { success: result.success, data: result.success ? result.data.name : "Anonymous" };
      },
      'Identifying...',
      (result: { success: boolean, data: string }) => {
        return result.success ? "Identify Succeeded" : "Identify Failed"
      },
      (result: { success: boolean, data: string }) => {
        setIdentityHeaderStable(result.data);
        return true;
      }
    );
  }

  //
  // Context menu actions
  //
  const actions = isAuthenticated
    ? [
      { label: 'Logout', variant: 'destructive', onClick: handleLogout },
    ]
    : [
      { label: 'Login with Auth0', variant: 'primary', onClick: handleLogin },
    ];

  //
  // Header class (color coding)
  //
  const headerClass = '';

  return (
    <IdentityContext.Provider
      value={{
        isAuthenticated,
        name: identityHeaderStable,
      }}
    >
      <html lang="en">
        <body className="app-root">
          <header className="app-header">
            <div className="header-left">
              <h1 className="app-title">Camp Fit Fur Dogs</h1>
            </div>

            <div className="header-right">
              <ActionsCard
                header={
                  <span className={`identity-header ${headerClass}`}>
                    <span className="identity-header-symbol">👤</span>

                    <span className="identity-header-text">
                      <span ref={resultLayerRef} className="layer-result opaque"></span>
                      <span ref={loadingLayerRef} className="layer-loading transparent"></span>
                      <span ref={statusLayerRef} className="layer-status transparent"></span>
                    </span>

                    <span className="identity-header-indicator">▾</span>
                  </span>
                }
                actions={actions}
              />
            </div>
          </header>

          <main className="app-main">{children}</main>
        </body>
      </html>
    </IdentityContext.Provider>
  );
}
