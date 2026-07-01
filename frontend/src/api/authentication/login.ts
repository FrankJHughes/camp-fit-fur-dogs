import { createApiClient } from '@/lib/api/client';
import { QueryResult } from "@/lib/api/queryResult";
import { LoginResponse } from "@/lib/authentication/getLoginResponse";

const client = createApiClient();

export async function login(returnUrl: string): Promise<QueryResult<LoginResponse>> {
  try {
    const urlSearchParams = new URLSearchParams();
    urlSearchParams.set('return_url', returnUrl);

    const url = '/auth/login?' + urlSearchParams.toString();

    const result = await client.get<LoginResponse>(url);

    if (result.ok) {
      return { success: true, data: result.data };
    }
    if (result.error.status === 404) {
      return { success: false, notFound: true };
    }
    return { success: false, notFound: false, error: result.error.message };
  } catch (err: any) {
    return { success: false, notFound: false, error: err.message };
  }
}
