const puppeteer = require('puppeteer');
const path = require('path');

(async () => {
    console.log("🚀 Démarrage de la génération de l'image OG...");

    const browser = await puppeteer.launch({
        headless: "new",
        ignoreHTTPSErrors: true // Nécessaire pour localhost https
    });
    const page = await browser.newPage();

    // Taille standard Open Graph
    await page.setViewport({ width: 1200, height: 630, deviceScaleFactor: 2 }); // Scale factor 2 pour la haute définition (Retina)

    // URL de l'application locale (basé sur launchSettings.json)
    const url = 'https://localhost:7344/og-image';

    console.log(`🌍 Navigation vers ${url}...`);
    
    try {
        await page.goto(url, { waitUntil: 'domcontentloaded' });
        
        // Attendre que le titre soit visible pour confirmer le chargement
        await page.waitForSelector('h1');
        
        // Petite pause pour laisser le temps aux polices et au flou de se charger correctement
        await new Promise(r => setTimeout(r, 1000));

        const outputPath = path.join(__dirname, 'wwwroot', 'og-image.png');
        
        await page.screenshot({ 
            path: outputPath,
            omitBackground: true
        });

        console.log(`✅ Image générée avec succès : ${outputPath}`);
    } catch (error) {
        console.error("❌ Erreur lors de la génération.");
        console.error("👉 Assurez-vous que votre application est lancée (dotnet watch ou dotnet run).");
        console.error(error);
    }

    await browser.close();
})();