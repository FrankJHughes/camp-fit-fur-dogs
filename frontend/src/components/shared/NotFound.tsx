interface NotFoundLink {
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
              <a href={link.href}>{link.label}</a>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}