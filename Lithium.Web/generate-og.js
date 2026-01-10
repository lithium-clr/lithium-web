const puppeteer = require('puppeteer');
const path = require('path');

(async () => {
    console.log("🚀 Starting OG image generation...");

    const browser = await puppeteer.launch({
        headless: "new",
        ignoreHTTPSErrors: true // Required for HTTPS localhost
    });
    const page = await browser.newPage();

    // Standard Open Graph size
    await page.setViewport({ width: 1200, height: 630, deviceScaleFactor: 2 }); // Scale factor 2 for high definition (Retina)

    // Local app URL (based on launchSettings.json)
    const url = 'https://localhost:7344/og-image';

    console.log(`🌍 Navigating to ${url}...`);

    try {
        await page.goto(url, { waitUntil: 'domcontentloaded' });

        // Wait for the title to be visible to confirm loading
        await page.waitForSelector('h1');

        // Small delay to allow fonts and blur effects to load properly
        await new Promise(r => setTimeout(r, 1000));

        const outputPath = path.join(__dirname, 'wwwroot', 'og-image.png');

        await page.screenshot({
            path: outputPath,
            omitBackground: true
        });

        console.log(`✅ Image successfully generated: ${outputPath}`);
    } catch (error) {
        console.error("❌ Error during generation.");
        console.error("👉 Make sure your application is running (dotnet watch or dotnet run).");
        console.error(error);
    }

    await browser.close();
})();
