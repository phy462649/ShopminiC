# ServiceMassage - Frontend SPA

React frontend application for the ServiceMassage system.

## 🚀 Getting Started

### Prerequisites

- Node.js 18+
- npm or yarn

### Installation

```bash
# Install dependencies
npm install

# Start development server
npm run dev

# Build for production
npm run build

# Preview production build
npm run preview
```

### Available Scripts

- `npm run dev` - Start development server
- `npm run build` - Build for production
- `npm run preview` - Preview production build
- `npm run lint` - Run ESLint

## 🛠️ Tech Stack

- **React 18** - UI framework
- **Vite** - Build tool and dev server
- **Tailwind CSS** - Utility-first CSS framework
- **ESLint** - Code linting

## 📁 Project Structure

```
Spa/
├── public/                 # Static assets
├── src/
│   ├── components/         # Reusable components
│   ├── pages/             # Page components
│   ├── hooks/             # Custom hooks
│   ├── services/          # API services
│   ├── utils/             # Utility functions
│   ├── styles/            # Global styles
│   └── main.jsx           # Application entry point
├── index.html             # HTML template
├── package.json           # Dependencies and scripts
├── tailwind.config.js     # Tailwind configuration
├── vite.config.js         # Vite configuration
└── eslint.config.js       # ESLint configuration
```

## 🔗 API Integration

The frontend communicates with the ASP.NET Core API backend. Make sure the API is running on the configured endpoint.

Default API endpoint: `https://localhost:5001/api`

## 🎨 Development Guidelines

- Use functional components with hooks
- Follow component naming conventions (PascalCase)
- Implement proper error handling
- Use Tailwind CSS for styling
- Follow React best practices

## 🤝 Contributing

Please follow the main project's contribution guidelines.