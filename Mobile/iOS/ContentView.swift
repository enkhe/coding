// SwiftUI root view with a NavigationStack and value-based destinations.
// Targets iOS 18, Swift 6.
import SwiftUI

struct ContentView: View {
    enum Route: Hashable { case weather, about }

    var body: some View {
        NavigationStack {
            List {
                NavigationLink("Weather", value: Route.weather)
                NavigationLink("About", value: Route.about)
            }
            .navigationTitle("Cheatsheet")
            .navigationDestination(for: Route.self) { route in
                switch route {
                case .weather: WeatherView()
                case .about: AboutView()
                }
            }
        }
    }
}

private struct AboutView: View {
    @Environment(\.dismiss) private var dismiss
    var body: some View {
        VStack(spacing: 16) {
            Text("Built with SwiftUI + Swift 6.")
            Button("Close", action: { dismiss() })
        }
        .padding()
    }
}

#Preview { ContentView() }
