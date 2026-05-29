export interface InitiateLoginResponse {
  redirectUrl: string;
}

export async function initiateLogin(): Promise<InitiateLoginResponse> {
  const response = await fetch("/api/auth/login", {
    method: "GET",
    credentials: "include",
  });

  if (!response.ok) {
    throw new Error(`Login initiation failed: ${response.status}`);
  }

  const data = (await response.json()) as InitiateLoginResponse;

  if (!data.redirectUrl) {
    throw new Error("Login initiation response missing redirectUrl");
  }

  return data;
}
