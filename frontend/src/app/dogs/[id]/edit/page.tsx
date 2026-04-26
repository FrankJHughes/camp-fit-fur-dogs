'use client';

import { useParams, useRouter } from 'next/navigation';
import { getDogProfile } from '@/api/dogs/getDogProfile';
import { toQueryState } from '@/lib/api/queryResult';
import { DogNotFound } from '@/components/dogs/DogNotFound';
import { editDogProfile, type EditDogProfileData } from '@/api/dogs/editDogProfile';
import { EditDogProfileForm } from '@/components/dogs/EditDogProfileForm';
import { useApiQuery } from '@/lib/hooks/useApiQuery';
import { useCommand } from '@/lib/hooks/useCommand';

export default function EditDogProfilePage() {
  const { id } = useParams<{ id: string }>();
  const router = useRouter();

  const state = useApiQuery(
    () => getDogProfile(id).then(toQueryState),
    [id]
  );

  const { errors, isSubmitting, handleSubmit } = useCommand<EditDogProfileData>(
    (data) => editDogProfile(id, data),
    () => router.push(`/dogs/${id}`)
  );

  if (state.status === 'loading') return <p>Loading…</p>;
  if (state.status === 'not-found') return <DogNotFound />;
  if (state.status === 'error') return <p>{state.error}</p>;

  return (
    <EditDogProfileForm
      initialData={{
        name: state.data.name,
        breed: state.data.breed,
        dateOfBirth: state.data.dateOfBirth,
        sex: state.data.sex,
      }}
      onSubmit={handleSubmit}
      errors={errors}
      isSubmitting={isSubmitting}
    />
  );
}