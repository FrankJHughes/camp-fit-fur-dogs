'use client';

export default function PublicLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <div className="public-shell">
      <main>{children}</main>
    </div>
  );
}
