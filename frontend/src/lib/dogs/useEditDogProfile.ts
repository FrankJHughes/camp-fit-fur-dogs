'use client';

import { useRouter } from 'next/navigation';
import { editDogProfile } from '@/api/dogs/editDogProfile';
import {
  type DogFormValues,
  mapDogFormValuesToEditCommand,
} from '@/lib/dogs/dogModel';
import { useFormCommand } from '@/lib/forms/useFormCommand';

export function useEditDogProfile(id: string) {
  const router = useRouter();

  const command = useFormCommand<DogFormValues>({
    submit: async (values) => {
      const cmd = mapDogFormValuesToEditCommand(values);
      return await editDogProfile(id, cmd);
    },
    onSuccess: () => router.push(`/dogs/${id}`),
  });

  return { command };
}
