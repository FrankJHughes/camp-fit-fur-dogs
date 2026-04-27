import Link from 'next/link';

interface DogItem {
  id: string;
  name: string;
  breed: string;
}

interface ListDogsByCurrentUserCardProps {
  dogs: DogItem[];
}

export function ListDogsByCurrentUserCard({ dogs }: ListDogsByCurrentUserCardProps) {
  return (
    <div>
      <h1>My Dogs</h1>
      {dogs.length === 0 ? (
        <p>No dogs registered yet.</p>
      ) : (
        <ul>
          {dogs.map((dog) => (
            <li key={dog.id}>
              <Link href={`/dogs/${dog.id}`}>
                {dog.name} — {dog.breed}
              </Link>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}
