using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Grappin : MonoBehaviour
{
    public MeshRenderer Grappin_s_Visual;
    public RawImage ShowCross;
    public RectTransform tankBar;
    public RectTransform cooldownBar;
    public Transform Grappin_s;
    public Transform MainCamera;
    public Transform playerTransform;
    public Rigidbody playerRigidbody;
    public Variables GeneralVar; // "PlayerState: 0 = mid-air, 1 = walk, 2 = wall-running
    public float grappin_sStreng;
    public float grappin_sBake;
    public float range;
    public float tank;
    public float cooldown;
    
    private bool _tract ;
    private float _cooldown_state = 0;
    private int _tank_size = 255;
    private float _bake_range;
    private bool _bake = false;
    private Vector3 _catchPoint;
    private bool _actif = false;
    private bool _first = true;
    private bool _bake_catch = false;
    private float _History_speed_angle;
    private bool _unflooring = true;
    private bool _backing = false;
    
    // Start is called before the first frame update
    void Start()
    {
        ShowCross.enabled = false;
        Grappin_s_Visual.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        var direction = MainCamera.rotation * Vector3.forward;
        var possible = ((Physics.Raycast(MainCamera.position, direction,
            out var hitInfo, range)) && (_cooldown_state<0));
        ShowCross.enabled = possible;
        if (Input.GetMouseButtonDown(0))
        {
            if (_first)
            {
                _first = false;
            }
            else
            {
                if (possible)
                {
                    _actif = true;
                    _catchPoint = hitInfo.point;
                    _cooldown_state = cooldown;
                }
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            if (_actif)
            {
                Grappin_s_Visual.enabled = false;
                _actif = false;
            }
        }

        if (_actif)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                _tract = true;
            }

            if (Input.GetKey(KeyCode.Q))
            {
                _bake = true;
            }
        }
    }

    private void Bake_now(float dist, Vector3 direction_normalized)
    {
        if (!_bake_catch)
        { 
            _bake_range = dist;
            _bake_catch = true;
        }
        if (dist > _bake_range)
        {
            float scal = Vector3.Dot(direction_normalized, playerRigidbody.velocity);
            if (scal < 0)
            {
                _backing = true;
                if ((-scal*1.2f) < (grappin_sBake*Time.deltaTime))
                {
                    playerRigidbody.AddForce(-scal * 1.2f * direction_normalized, ForceMode.Impulse);

                }
                else
                {
                    playerRigidbody.AddForce((grappin_sBake+(grappin_sStreng*0.5f)) * direction_normalized, ForceMode.Acceleration);
                    if (dist < range)
                    {
                        _bake_range = dist;
                    }
                }
            }
        }
        if (dist > (range * 1.1f))
        {
            Grappin_s_Visual.enabled = false;
            _actif = false;
        }
    }
    private void FixedUpdate()
    {
        var genVar_declaration = GeneralVar.declarations;
        if (_actif)
        {
            if (!Grappin_s_Visual.enabled)
            {
                Grappin_s_Visual.enabled = true;
            }
            var pointA = playerTransform.position + (Vector3.up * 0.8f);
            var direction_vetor = _catchPoint - pointA;
            var directionH = (Vector3.forward * direction_vetor.z) + (Vector3.right * direction_vetor.x);
            pointA = pointA + (directionH.normalized * 0.6f);
            direction_vetor = _catchPoint - pointA;
            var dist = direction_vetor.magnitude;
            var direction_normalized = direction_vetor.normalized;
            Grappin_s.position = pointA;
            Grappin_s.rotation = Quaternion.LookRotation(direction_normalized);
            Grappin_s.localScale = Vector3.one + (Vector3.forward * (dist-0.8f));
            if (_bake)
            {
                _bake = false;
                Bake_now(dist, direction_normalized);
            }
            else
            {
                if (dist > range)
                {
                    Bake_now(dist, direction_normalized);
                }
                else
                {
                    _bake_catch = false;
                }

                if (_tract)
                {
                    _tract = false;
                    if (tank > 0)
                    {
                        genVar_declaration.Set("playerFlying", true);
                        var vitesse = Vector3.Dot(direction_normalized, playerRigidbody.velocity);
                        if (vitesse < 10f)
                        {
                            playerRigidbody.AddForce((grappin_sStreng * direction_normalized),
                                ForceMode.Acceleration);
                            tank -= Time.deltaTime*10;
                        }
                    }
                    else
                    {
                        tank = 0;
                    }
                }
            }
        }
        var state = genVar_declaration.Get("PlayerState");
        if (state.Equals(0) & _actif & (_backing | _tract))
        {
            var temp = Quaternion.LookRotation(playerRigidbody.velocity).eulerAngles.y;
            if (_unflooring)
            {
                _unflooring = false;
            }
            else
            {
                var rotation = temp - _History_speed_angle;
                if ((-2.5f < rotation) & (rotation < 2.5f))
                {
                    playerTransform.Rotate(Vector3.up, rotation);
                }
            }

            _History_speed_angle = temp;
        }
        else
        {
            _unflooring = true;
        }

        _cooldown_state -= Time.deltaTime;
        var cooldownTemp = cooldown - ((_cooldown_state>0)?_cooldown_state:0);
        cooldownBar.anchoredPosition = (Vector2.right * (cooldownTemp * 250 / cooldown)) + (Vector2.up * cooldownBar.anchoredPosition.y);
        cooldownBar.sizeDelta = (Vector2.right * (cooldownTemp * 500 / cooldown)) + (Vector2.up * cooldownBar.sizeDelta.y);
        tankBar.anchoredPosition = Vector2.right * (tank * 250 / _tank_size);
        tankBar.sizeDelta = (Vector2.right * (tank * 500 / _tank_size)) + (Vector2.up * tankBar.sizeDelta.y);
        _backing = false;
    }
}