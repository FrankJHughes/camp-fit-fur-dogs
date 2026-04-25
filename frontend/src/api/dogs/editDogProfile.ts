import { createApiClient } from '@/lib/api/client';

export interface EditDogProfileData {
  name: string;
  breed: string;
  dateOfBirth: string;
  sex: string;
}

export interface EditDogProfileResult {
  success: boolean;
  errors?: Record<string, string>;
}

const client = createApiClient();

export async function editDogProfile(
  dogId: string,
  data: EditDogProfileData
): Promise<EditDogProfileResult> {
  const result = await client.put<void>(`/dogs/${dogId}`, data);
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