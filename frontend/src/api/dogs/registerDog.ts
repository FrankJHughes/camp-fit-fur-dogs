import { createApiClient } from '@/lib/api/client';
import type { RegisterDogFormData } from '@/components/dogs/RegisterDogForm';

export interface RegisterDogResult {
  success: boolean;
  errors?: Record<string, string>;
}

const client = createApiClient();

export async function registerDog(data: RegisterDogFormData): Promise<RegisterDogResult> {
  const result = await client.post<void>('/dogs/register', data);
  if (result.ok) {
    return { success: true };
  }
  if (result.error.errors) {
    const errors = Object.fromEntries(
      Object.entries(result.error.errors).map(([k, v]) =>
        [k, Array.isArray(v) ? v[0] : v]
      )
    );
    return { success: false, errors };
  }
  const message = result.error.type === 'network'
    ? 'A network error occurred. Please try again.'
    : result.error.message;
  return { success: false, errors: { form: message } };
}