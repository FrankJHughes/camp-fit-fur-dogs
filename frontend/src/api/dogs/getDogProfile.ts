import { createApiClient } from '@/lib/api/client';

export interface DogProfile {
  id: string;
  ownerId: string;
  name: string;
  breed: string;
  dateOfBirth: string;
  sex: string;
}

export type GetDogProfileResult =
  | { success: true; profile: DogProfile }
  | { success: false; notFound: boolean; error?: string };

const client = createApiClient();

export async function getDogProfile(dogId: string): Promise<GetDogProfileResult> {
  const result = await client.get<DogProfile>(`/dogs/${dogId}`);
  if (result.ok) {
    return { success: true, profile: result.data };
  }
  if (result.error.status === 404) {
    return { success: false, notFound: true };
  }
  return { success: false, notFound: false, error: result.error.message };
}