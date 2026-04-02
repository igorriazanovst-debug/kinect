$kinectPath   = "C:\Temp\2026\kinect\kinect\My project\Assets\Scripts\Kinect"
$gesturesPath = "C:\Temp\2026\kinect\kinect\My project\Assets\Scripts\Gestures"

# ---- Fix KinectManager.cs Update() ----
$oldUpdate = @'
        private void Update()
        {
            if (!IsConnected)
            {
                // Для UDP — проверяем появление данных
                if (_provider is UDPKinectProvider)
                {
                    var data = _provider.GetPrimarySkeleton();
                    if (data != null && data.IsTracked)
                    {
                        IsConnected = true;
                        EventBus.Publish(GameEvents.KinectConnected);
                    }
                }
                return;
            }

            PrimaryPlayer = _provider.GetPrimarySkeleton();
            if (PrimaryPlayer != null && PrimaryPlayer.IsTracked)
                EventBus.Publish(GameEvents.PlayerDetected, PrimaryPlayer);
            else
                EventBus.Publish(GameEvents.PlayerLost);
        }
'@

$newUpdate = @'
        private void Update()
        {
            if (_provider == null) return;

            if (!IsConnected)
            {
                if (_provider is UDPKinectProvider)
                {
                    var data = _provider.GetPrimarySkeleton();
                    if (data != null && data.IsTracked)
                    {
                        IsConnected = true;
                        EventBus.Publish(GameEvents.KinectConnected);
                    }
                }
                return;
            }

            PrimaryPlayer = _provider.GetPrimarySkeleton();
            if (PrimaryPlayer != null && PrimaryPlayer.IsTracked)
                EventBus.Publish(GameEvents.PlayerDetected, PrimaryPlayer);
            else
                EventBus.Publish(GameEvents.PlayerLost);
        }
'@

$kmPath = "$kinectPath\KinectManager.cs"
$km = Get-Content $kmPath -Raw
$km = $km.Replace($oldUpdate, $newUpdate)
Set-Content $kmPath $km -Encoding UTF8
Write-Host "KinectManager.cs fixed."

# ---- Fix GestureManager.cs Awake() ----
$oldAwake = @'
        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;

            foreach (GestureType g in System.Enum.GetValues(typeof(GestureType)))
            {
                _holdTimers[g] = 0f;
                _coolTimers[g] = 0f;
            }
        }
'@

$newAwake = @'
        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;

            foreach (GestureType g in System.Enum.GetValues(typeof(GestureType)))
            {
                if (g == GestureType.None) continue;
                _holdTimers[g] = 0f;
                _coolTimers[g] = 0f;
            }
        }
'@

# Fix TickCooldowns
$oldTick = @'
        private void TickCooldowns()
        {
            foreach (GestureType g in System.Enum.GetValues(typeof(GestureType)))
            {
                if (_coolTimers[g] > 0f)
                    _coolTimers[g] -= Time.deltaTime;
            }
        }
'@

$newTick = @'
        private void TickCooldowns()
        {
            foreach (GestureType g in System.Enum.GetValues(typeof(GestureType)))
            {
                if (g == GestureType.None) continue;
                if (_coolTimers[g] > 0f)
                    _coolTimers[g] -= Time.deltaTime;
            }
        }
'@

$gmPath = "$gesturesPath\GestureManager.cs"
$gm = Get-Content $gmPath -Raw
$gm = $gm.Replace($oldAwake, $newAwake)
$gm = $gm.Replace($oldTick,  $newTick)
Set-Content $gmPath $gm -Encoding UTF8
Write-Host "GestureManager.cs fixed."
