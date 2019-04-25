<?php

namespace App;

use Illuminate\Notifications\Notifiable;
use Illuminate\Foundation\Auth\User as Authenticatable;
use Tymon\JWTAuth\Contracts\JWTSubject;

class User extends Authenticatable implements JWTSubject
{
    use Notifiable;

    protected $table = 'Users';
    public $primaryKey = 'nickname';
    public $timestamps = false;

    protected $fillable = [
        'nickname', 'password', 'profileDescription', 'profileLocation', 'birthDate', 'privateProfile', 'accountCreationDate', 'admin', 'bannedUntil', 'banReason', 'online',
    ];

    protected $hidden = [
        'password',
    ];

    protected $casts = [
        'bannedUntil' => 'datetime', 'birthDate' => 'date', 'accountCreationDate' => 'date',
    ];

    public function getJWTIdentifier()
    {
        return $this->getKey();
    }

    public function getJWTCustomClaims()
    {
        return [];
    }
}
