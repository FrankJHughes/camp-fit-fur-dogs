// src/lib/dogs/useEditDogProfile.ts
'use client';

import { useRouter } from 'next/navigation';
import { editDogProfile } from '@/api/dogs/editDogProfile';
import { useFormCommand } from '@/lib/forms/useFormCommand';
import type { DogFormValues, EditDogProfileCommand } from '@/lib/dogs/dogModel';
import { mapDogFormValuesToEditCommand } from '@/lib/dogs/dogModel';

export function useEditDogProfile(id: string) {
  const router = useRouter();

  const command = useFormCommand<DogFormValues>({
    run: (values: DogFormValues) => {
      const cmd: EditDogProfileCommand = mapDogFormValuesToEditCommand(values);
      return editDogProfile(id, cmd);
    },
    onSuccess: () => router.push(`/dogs/${id}`),
  });

  return { command };
}
