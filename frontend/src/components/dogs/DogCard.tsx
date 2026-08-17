import type { Dog } from '@/lib/dogs/dogModel';

interface DogCardProps {
  profile: Dog;
}

export function DogCard({ profile }: DogCardProps) {
  const formattedDob = new Date(profile.dateOfBirth + 'T00:00:00').toLocaleDateString(
    'en-US',
    { year: 'numeric', month: 'long', day: 'numeric' }
  );

  return (
    <section
      className="card"
      aria-labelledby="dog-profile-heading"
    >
      <h2 id="dog-profile-heading" className="card-title">
        {profile.name}
      </h2>

      <dl className="card-details">
        <div className="detail-row">
          <dt>Breed</dt>
          <dd>{profile.breed}</dd>
        </div>

        <div className="detail-row">
          <dt>Date of Birth</dt>
          <dd>{formattedDob}</dd>
        </div>

        <div className="detail-row">
          <dt>Sex</dt>
          <dd>{profile.sex}</dd>
        </div>
      </dl>
    </section>
  );
}
