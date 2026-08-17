// src/lib/dogs/useEditDog.ts
'use client';

import { useRouter } from 'next/navigation';
import { editDog } from '@/api/dogs/editDog';
import { useFormCommand } from '@/lib/forms/useFormCommand';
import type { DogFormValues, EditDogCommand } from '@/lib/dogs/dogModel';
import { mapDogFormValuesToEditCommand } from '@/lib/dogs/dogModel';

export function useEditDog(id: string) {
  const router = useRouter();

  const command = useFormCommand<DogFormValues>({
    run: (values: DogFormValues) => {
      const cmd: EditDogCommand = mapDogFormValuesToEditCommand(values);
      return editDog(id, cmd);
    },
    onSuccess: () => router.push(`/dogs/${id}`),
  });

  return { command };
}
