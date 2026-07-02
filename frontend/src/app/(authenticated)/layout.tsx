'use client';

import { useIdentity } from '@/lib/identity/useIdentity';

export default function AuthenticatedLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  const { isAuthenticated } = useIdentity();

  return (
    <div className="authenticated-shell">
      <main>
        {isAuthenticated ? (
          children
        ) : (
          <p style={{ padding: '2rem', fontSize: '1.25rem' }}>
            Login to view this page
          </p>
        )}
      </main>
    </div>
  );
}
