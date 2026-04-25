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

export async function getDogProfile(dogId: string): Promise<GetDogProfileResult> {
  try {
    const response = await fetch(`/api/dogs/${dogId}`);

    if (!response.ok) {
      if (response.status === 404) {
        return { success: false, notFound: true };
      }
      return {
        success: false,
        notFound: false,
        error: 'An unexpected error occurred. Please try again.',
      };
    }

    const profile: DogProfile = await response.json();
    return { success: true, profile };
  } catch {
    return {
      success: false,
      notFound: false,
      error: 'A network error occurred. Please try again.',
    };
  }
}