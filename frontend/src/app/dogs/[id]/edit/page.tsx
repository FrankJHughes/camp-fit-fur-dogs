'use client';

import { useParams, useRouter } from 'next/navigation';
import { getDogProfile } from '@/api/dogs/getDogProfile';
import { toQueryState } from '@/lib/api/queryResult';
import { DogNotFound } from '@/components/dogs/DogNotFound';
import { editDogProfile } from '@/api/dogs/editDogProfile';
import { EditDogProfileForm } from '@/components/dogs/EditDogProfileForm';
import { useApiQuery } from '@/lib/hooks/useApiQuery';
import { useFormCommand } from '@/lib/forms/useFormCommand';
import {
  type DogFormValues,
  mapDogFormValuesToEditCommand,
} from '@/lib/dogs/dogModel';

export default function EditDogProfilePage() {
  const { id } = useParams<{ id: string }>();
  const router = useRouter();

  const state = useApiQuery(
    () => getDogProfile(id).then(toQueryState),
    [id]
  );

  const command = useFormCommand<DogFormValues>({
    submit: async (values) => {
      const cmd = mapDogFormValuesToEditCommand(values);
      return await editDogProfile(id, cmd);
    },
    onSuccess: () => router.push(`/dogs/${id}`),
  });

  if (state.status === 'loading') return <p>Loading…</p>;
  if (state.status === 'not-found') return <DogNotFound />;
  if (state.status === 'error') return <p>{state.error}</p>;

  const initialValues: DogFormValues = {
    name: state.data.name,
    breed: state.data.breed,
    dateOfBirth: state.data.dateOfBirth,
    sex: state.data.sex as '' | 'Male' | 'Female',
  };

  return (
    <EditDogProfileForm
      command={command}
      initialValues={initialValues}
    />
  );
}
