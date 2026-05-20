// /api/account/createAccount.ts

export interface CreateAccountCommand {
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  password: string;
}

export async function createAccount(cmd: CreateAccountCommand) {
  const res = await fetch("/api/customers", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(cmd),
  });

  const json = await res.json();

  if (!res.ok) {
    return {
      ok: false as const,
      error: json,
    };
  }

  return {
    ok: true as const,
    data: json,
  };
}
