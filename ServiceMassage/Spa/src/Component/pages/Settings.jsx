export default function Settings() {
  return (
    <div className="p-4 text-xl">
      <h1 className="mb-4">Settings Content</h1>
      <div className="space-y-2">
        {Array.from({ length: 200 }).map((_, i) => (
          <div key={i} className="p-4 bg-gray-100 rounded border shadow-sm">
            Item {i + 1}
          </div>
        ))}
      </div>
    </div>
  );
}
