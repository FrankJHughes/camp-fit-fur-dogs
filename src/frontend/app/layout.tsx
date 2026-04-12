import type { Metadata } from 'next';
import './globals.css';

export const metadata: Metadata = {
  title: 'Camp Fit Fur Dogs',
  description: 'Dog fitness camp management system',
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="en">
      <body>{children}</body>
    </html>
  );
}
