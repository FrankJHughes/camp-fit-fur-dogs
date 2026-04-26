'use client';

import { useParams, useRouter } from 'next/navigation';
import { getDogProfile, type DogProfile } from '@/api/dogs/getDogProfile';
import { editDogProfile, type EditDogProfileData } from '@/api/dogs/editDogProfile';
import { EditDogProfileForm } from '@/components/dogs/EditDogProfileForm';
import { useApiQuery, type QueryState } from '@/lib/hooks/useApiQuery';
import { useCommand } from '@/lib/hooks/useCommand';

function toQueryState(result: Awaited<ReturnType<typeof getDogProfile>>): QueryState<DogProfile> {
  if (result.success) return { status: 'success', data: result.profile };
  if (result.notFound) return { status: 'not-found' };
  return { status: 'error', error: result.error };
}

export default function EditDogProfilePage() {
  const { id } = useParams<{ id: string }>();
  const router = useRouter();

  const state = useApiQuery<DogProfile>(
    () => getDogProfile(id).then(toQueryState),
    [id]
  );

  const { errors, isSubmitting, handleSubmit } = useCommand<EditDogProfileData>(
    (data) => editDogProfile(id, data),
    () => router.push(`/dogs/${id}`)
  );

  if (state.status === 'loading') return <p>Loading…</p>;
  if (state.status === 'not-found') return <p>Dog not found.</p>;
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