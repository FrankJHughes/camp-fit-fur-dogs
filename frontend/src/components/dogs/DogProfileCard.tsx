import type { DogProfile } from '@/api/dogs/getDogProfile';
import type { ReactNode } from 'react';

interface DogProfileCardProps {
  profile: DogProfile;
  actions?: ReactNode;
}

export function DogProfileCard({ profile }: DogProfileCardProps) {
  const formattedDob = new Date(profile.dateOfBirth + 'T00:00:00').toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'long',
    day: 'numeric',
  });

  return (
    <div>
      <h2>{profile.name}</h2>
      <dl>
        <dt>Breed</dt>
        <dd>{profile.breed}</dd>
        <dt>Date of Birth</dt>
        <dd>{formattedDob}</dd>
        <dt>Sex</dt>
        <dd>{profile.sex}</dd>
      </dl>
    </div>
  );
}
