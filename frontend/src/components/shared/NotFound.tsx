import Link from 'next/link';
export interface NotFoundLink {
  label: string;
  href: string;
}

interface NotFoundProps {
  heading: string;
  message: string;
  links?: NotFoundLink[];
}

export function NotFound({ heading, message, links = [] }: NotFoundProps) {
  return (
    <div>
      <h1>{heading}</h1>
      <p>{message}</p>
      {links.length > 0 && (
        <ul>
          {links.map((link) => (
            <li key={link.href}>
              <Link href={link.href}>{link.label}</Link>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}
