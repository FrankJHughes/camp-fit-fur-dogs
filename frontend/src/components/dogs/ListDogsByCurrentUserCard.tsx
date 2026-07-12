import Link from 'next/link';
import type { DogListItem } from '@/lib/dogs/dogModel';

interface ListDogsByCurrentUserCardProps {
  dogs: DogListItem[];
}

export function ListDogsByCurrentUserCard({ dogs }: ListDogsByCurrentUserCardProps) {
  return (
    <section className="card" aria-labelledby="my-dogs-heading">
      <h2 id="my-dogs-heading" className="card-title">My Dogs</h2>

      {dogs.length === 0 ? (
        <p className="empty-state">No dogs registered yet.</p>
      ) : (
        <ul className="dog-list">
          {dogs.map((dog) => (
            <li key={dog.id} className="dog-list-item">
              <Link
                href={`/api/dogs/${dog.id}`}
                className="dog-list-link"
              >
                <span className="dog-name">{dog.name}</span>
                <span className="dog-breed"> — {dog.breed}</span>
              </Link>
            </li>
          ))}
        </ul>
      )}
    </section>
  );
}
