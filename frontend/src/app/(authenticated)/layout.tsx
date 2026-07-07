'use client';

import { useIdentityState } from '@/lib/identity/useIdentityState';

export default function AuthenticatedLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  const { isAuthenticated } = useIdentityState();

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
