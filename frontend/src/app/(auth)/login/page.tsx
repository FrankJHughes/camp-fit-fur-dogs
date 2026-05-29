"use client";

import { useLogin } from "@/lib/auth/useLogin";

export default function LoginPage() {
  const { login, loading } = useLogin();

  return (
    <main>
      <h1>Login</h1>
      <button onClick={login} disabled={loading}>
        {loading ? "Redirecting…" : "Login"}
      </button>
    </main>
  );
}
