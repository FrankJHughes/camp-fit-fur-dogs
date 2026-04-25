'use client';
import type { EditDogProfileData } from '@/api/dogs/editDogProfile';

import { useState } from 'react';

interface EditDogProfileFormProps {
    initialData: EditDogProfileData;
    onSubmit: (data: EditDogProfileData) => void;
    errors?: Record<string, string>;
    isSubmitting?: boolean;
}

export function EditDogProfileForm({
    initialData,
    onSubmit,
    errors,
    isSubmitting,
}: EditDogProfileFormProps) {
    const [name, setName] = useState(initialData.name);
    const [breed, setBreed] = useState(initialData.breed);
    const [dateOfBirth, setDateOfBirth] = useState(initialData.dateOfBirth);
    const [sex, setSex] = useState(initialData.sex);
    const [validationErrors, setValidationErrors] = useState<
        Record<string, string>
    >({});

    const displayErrors = { ...validationErrors, ...errors };

    const handleSubmit = () => {
        const newErrors: Record<string, string> = {};
        if (!name.trim()) newErrors.name = 'Name is required';
        if (!breed.trim()) newErrors.breed = 'Breed is required';
        if (!dateOfBirth.trim())
            newErrors.dateOfBirth = 'Date of birth is required';
        if (!sex) newErrors.sex = 'Sex is required';

        if (Object.keys(newErrors).length > 0) {
            setValidationErrors(newErrors);
            return;
        }

        setValidationErrors({});
        onSubmit({ name, breed, dateOfBirth, sex });
    };

    return (
        <form onSubmit={(e) => { e.preventDefault(); handleSubmit(); }}>
            <h1>Edit Dog Profile</h1>
            {displayErrors.form && <p>{displayErrors.form}</p>}

            <label>
                Name
                <input
                    type="text"
                    value={name}
                    onChange={(e) => setName(e.target.value)}
                />
            </label>
            {displayErrors.name && <p>{displayErrors.name}</p>}

            <label>
                Breed
                <input
                    type="text"
                    value={breed}
                    onChange={(e) => setBreed(e.target.value)}
                />
            </label>
            {displayErrors.breed && <p>{displayErrors.breed}</p>}

            <label>
                Date of Birth
                <input
                    type="text"
                    value={dateOfBirth}
                    onChange={(e) => setDateOfBirth(e.target.value)}
                />
            </label>
            {displayErrors.dateOfBirth && (
                <p>{displayErrors.dateOfBirth}</p>
            )}

            <label>
                Sex
                <select value={sex} onChange={(e) => setSex(e.target.value)}>
                    <option value="">Select</option>
                    <option value="Male">Male</option>
                    <option value="Female">Female</option>
                </select>
            </label>
            {displayErrors.sex && <p>{displayErrors.sex}</p>}

            <button type="submit" disabled={isSubmitting}>
                Save
            </button>
        </form>
    );
}
