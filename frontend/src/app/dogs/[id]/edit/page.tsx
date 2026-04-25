'use client';

import { useEffect, useState } from 'react';
import { useParams, useRouter } from 'next/navigation';
import { getDogProfile } from '@/api/dogs/getDogProfile';
import type { DogProfile } from '@/api/dogs/getDogProfile';
import {
    editDogProfile,
    type EditDogProfileData,
} from '@/api/dogs/editDogProfile';
import { EditDogProfileForm } from '@/components/dogs/EditDogProfileForm';

export default function EditDogProfilePage() {
    const { id } = useParams<{ id: string }>();
    const router = useRouter();

    const [profile, setProfile] = useState<DogProfile | null>(null);
    const [loading, setLoading] = useState(true);
    const [notFound, setNotFound] = useState(false);
    const [fetchError, setFetchError] = useState<string | undefined>();

    const [errors, setErrors] = useState<
        Record<string, string> | undefined
    >();
    const [isSubmitting, setIsSubmitting] = useState(false);

    useEffect(() => {
        getDogProfile(id).then((result) => {
            if (result.success) {
                setProfile(result.profile);
            } else {
                if (result.notFound) {
                    setNotFound(true);
                } else {
                    setFetchError(result.error);
                }
            }
            setLoading(false);
        });
    }, [id]);

    const handleSubmit = async (data: EditDogProfileData) => {
        setIsSubmitting(true);
        setErrors(undefined);

        const result = await editDogProfile(id, data);

        if (result.success) {
            router.push(`/dogs/${id}`);
        } else {
            setErrors(result.errors);
            setIsSubmitting(false);
        }
    };

    if (loading) return <p>Loading…</p>;
    if (notFound) return <p>Dog not found.</p>;
    if (fetchError) return <p>{fetchError}</p>;
    if (!profile) return null;

    return (
        <EditDogProfileForm
            initialData={{
                name: profile.name,
                breed: profile.breed,
                dateOfBirth: profile.dateOfBirth,
                sex: profile.sex,
            }}
            onSubmit={handleSubmit}
            errors={errors}
            isSubmitting={isSubmitting}
        />
    );
}
