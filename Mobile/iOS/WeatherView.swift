// Async fetch with the @Observable macro and the .task view modifier.
// Demonstrates Swift 6 strict concurrency: the store is @MainActor.
import SwiftUI

struct Reading: Decodable {
    let tempC: Double
    let summary: String
}

@MainActor
@Observable
final class WeatherStore {
    var reading: Reading?
    var isLoading = false
    var errorMessage: String?

    func refresh() async {
        isLoading = true
        defer { isLoading = false }
        do {
            // Replace with a real endpoint; this URL just illustrates the pattern.
            let url = URL(string: "https://example.com/weather.json")!
            let (data, _) = try await URLSession.shared.data(from: url)
            reading = try JSONDecoder().decode(Reading.self, from: data)
            errorMessage = nil
        } catch {
            errorMessage = error.localizedDescription
        }
    }
}

struct WeatherView: View {
    @State private var store = WeatherStore()

    var body: some View {
        VStack(spacing: 16) {
            if store.isLoading {
                ProgressView()
            } else if let r = store.reading {
                Text("\(r.tempC, specifier: "%.1f") C")
                    .font(.largeTitle)
                Text(r.summary)
                    .foregroundStyle(.secondary)
            } else if let msg = store.errorMessage {
                Text(msg).foregroundStyle(.red)
            } else {
                Text("Pull to refresh.")
            }

            Button("Refresh") {
                Task { await store.refresh() }
            }
        }
        .padding()
        .navigationTitle("Weather")
        .task { await store.refresh() }
    }
}

#Preview { NavigationStack { WeatherView() } }
