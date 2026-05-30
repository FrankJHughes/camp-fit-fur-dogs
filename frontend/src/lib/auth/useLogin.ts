"use client";

import { useState } from "react";
import { initiateLogin } from "@/lib/auth/login";

export function useLogin() {
  const [loading, setLoading] = useState(false);

  async function login() {
    setLoading(true);

    const result = await initiateLogin();

    if (result?.redirectUrl) {
      window.location.assign(result.redirectUrl);
    }
  }

  return { login, loading };
}
