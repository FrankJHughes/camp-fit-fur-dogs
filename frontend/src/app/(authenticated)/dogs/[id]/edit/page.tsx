'use client';

import { useParams, useRouter } from 'next/navigation';
import { getDog } from '@/api/dogs/getDog';
import { toQueryState } from '@/lib/api/queryResult';
import { DogNotFound } from '@/components/dogs/DogNotFound';
import { editDog } from '@/api/dogs/editDog';
import EditDogForm from '@/components/dogs/EditDogForm';
import { useApiQuery } from '@/lib/hooks/useApiQuery';
import { useFormCommand } from '@/lib/forms/useFormCommand';
import type { DogFormValues, EditDogCommand } from '@/lib/dogs/dogModel';
import { mapDogFormValuesToEditCommand } from '@/lib/dogs/dogModel';

export default function EditDogPage() {
  const { id } = useParams<{ id: string }>();
  const router = useRouter();

  const state = useApiQuery(
    () => getDog(id).then(toQueryState),
    [id]
  );

  const command = useFormCommand<DogFormValues>({
    run: (values: DogFormValues) => {
      const cmd: EditDogCommand = mapDogFormValuesToEditCommand(values);
      return editDog(id, cmd);
    },
    onSuccess: () => router.push(`/dogs/${id}`),
  });

  // Loading
  if (state.status === 'loading') {
    return <p>Loading…</p>;
  }

  // Unauthenticated
  if (state.status === 'unauthenticated') {
    return (
      <p role="alert" aria-live="assertive" className="error-message">
        You must be logged in to edit this dog.
      </p>
    );
  }

  // Not found
  if (state.status === 'not-found') {
    return <DogNotFound />;
  }

  // Error
  if (state.status === 'error') {
    return (
      <p role="alert" aria-live="assertive" className="error-message">
        An unexpected error occurred. Please try again.
      </p>
    );
  }

  // Success — safe to access state.data
  const initialValues: DogFormValues = {
    name: state.data.name,
    breed: state.data.breed,
    dateOfBirth: state.data.dateOfBirth,
    sex: state.data.sex as 'Male' | 'Female',
  };

  return (
    <main className="page-container">
      <h1 className="page-title">Edit Dog Profile</h1>

      <EditDogForm
        command={command}
        initialValues={initialValues}
      />
    </main>
  );
}
