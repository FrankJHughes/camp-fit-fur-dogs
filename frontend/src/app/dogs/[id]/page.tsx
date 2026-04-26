'use client';

import { useParams, useRouter } from 'next/navigation';
import { getDogProfile } from '@/api/dogs/getDogProfile';
import { toQueryState } from '@/lib/api/queryResult';
import { DogNotFound } from '@/components/dogs/DogNotFound';
import { DogProfileCard } from '@/components/dogs/DogProfileCard';
import { ActionsCard } from '@/components/shared/ActionsCard';
import { ConfirmDialog } from '@/components/shared/ConfirmDialog';
import { useRemoveDog } from '@/hooks/dogs/useRemoveDog';
import { useApiQuery } from '@/lib/hooks/useApiQuery';
import type { Action } from '@/lib/shared/action';

export default function GetDogProfilePage() {
    const { id } = useParams<{ id: string }>();
    const router = useRouter();

    const state = useApiQuery(
        () => getDogProfile(id).then(toQueryState),
        [id]
    );

    const removeDog = useRemoveDog(
        id,
        state.status === 'success' ? state.data.name : '',
        router.push,
    );

    if (state.status === 'loading') return <p>Loading…</p>;
    if (state.status === 'not-found') return <DogNotFound />;
    if (state.status === 'error') return <p>{state.error}</p>;

    const actions: Action[] = [
        { label: 'Edit', onClick: () => router.push(`/dogs/${id}/edit`) },
        { label: 'Remove', onClick: removeDog.open },
    ];

    return (
        <>
            <DogProfileCard profile={state.data} />
            <ActionsCard actions={actions} />
            <ConfirmDialog {...removeDog.dialogProps} />
        </>
    );
}
