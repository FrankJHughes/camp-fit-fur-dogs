import { NotFound } from '../../lib/components/NotFound';

export function DogNotFound() {
  return (
    <NotFound
      heading="We couldn't find that dog"
      message="They may have been removed, or the link might be outdated."
      links={[
        { label: 'View your dogs', href: '/dogs' },
        { label: 'Register a new dog', href: '/dogs/register' },
      ]}
    />
  );
}
