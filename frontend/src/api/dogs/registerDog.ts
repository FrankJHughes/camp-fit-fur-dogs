import type { RegisterDogFormData } from '@/components/dogs/RegisterDogForm';

export interface RegisterDogResult {
  success: boolean;
  errors?: Record<string, string>;
}

export async function registerDog(data: RegisterDogFormData): Promise<RegisterDogResult> {
  try {
    const response = await fetch('/api/dogs/register', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(data),
    });

    if (!response.ok) {
      try {
        const body = await response.json();
        return { success: false, errors: body.errors };
      } catch {
        return { success: false, errors: { form: 'An unexpected error occurred. Please try again.' } };
      }
    }

    return response.json();
  } catch {
    return { success: false, errors: { form: 'A network error occurred. Please try again.' } };
  }
}
